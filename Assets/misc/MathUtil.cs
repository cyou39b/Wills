using UnityEngine;

public static class MathUtil
{
    // Some useful constants
    public const float PI = Mathf.PI;
    public const float NPI = - Mathf.PI;
    public const float HalfPI = PI / 2;
    public const float HalfNPI = NPI / 2;
    
    /// <summary>
    /// Convert a Vector2 to Vector3.
    /// </summary>
    /// <param name="a">The input Vector2.</param>
    /// <param name="z">Optional field with 0.0f as default value, the z value of returned Vector3.</param>
    /// <returns>A new Vector3 which have the same x and y as the given Vector2.</returns>
    public static Vector3 Vector2ToVecotr3(Vector2 a, float z = 0.0f)
    {
        return new Vector3(a.x, a.y, z);
    }

    /// <summary>
    /// Generate a random point within a circle, one-pass and statistically fair.
    /// </summary>
    /// <param name="center">The center of the cirlce.</param>
    /// <param name="radius">Radius of the circle.</param>
    /// <returns>A point with distance to center less than or equal to radius.</returns>
    public static Vector2 RandomPointInCircle(Vector2 center, float radius)
    {
        float r = Mathf.Sqrt(Random.Range(0.0f, 1.0f)) * radius;
        float rot = Random.Range(NPI, PI);
        center.x += r * Mathf.Cos(rot);
        center.y += r * Mathf.Sin(rot);
        return center;
    }

    /// <summary>
    /// Generate a random point within a donut shape (formally referred as 2D Torus), this function is statistically fair and ONE-PASS!!
    /// </summary>
    /// <param name="center">The center of the donut.</param>
    /// <param name="small_radius">The radius of the inner circle.</param>
    /// <param name="large_radius">The radius of the outer circle.</param>
    /// <returns>A point with distance to the center in range [small_radius, large_radius].</returns>
    public static Vector2 RandomPointInDonut(Vector2 center, float small_radius, float large_radius)
    {
        small_radius /= large_radius;
        float r = Mathf.Sqrt(Random.Range(small_radius * small_radius, 1.0f)) * large_radius;
        float theta = Random.Range(NPI, PI);
        center.x += r*Mathf.Cos(theta);
        center.y += r*Mathf.Sin(theta);
        return center;
    }

    /// <summary>
    /// Add a Vector3 and a Vector2, do the type cast for you so you don't have to.
    /// </summary>
    /// <param name="v3">The Vector3.</param>
    /// <param name="v2">The Vector2.</param>
    /// <returns>The result of adding the two give vectors.</returns>
    public static Vector3 AddVectors(Vector3 v3, Vector2 v2)
    {
        v3.x += v2.x;
        v3.y += v2.y;
        return v3;
    }

    /// <summary>
    /// Solves a quadratic formula.
    /// </summary>
    /// <param name="a">The coefficient of quadratic term.</param>
    /// <param name="b">The coefficient of linear term.</param>
    /// <param name="c">The coefficient of zero-degree term.</param>
    /// <returns>The roots of the quadratic. In the case of equal roots, root1 will be equal to root2. If there is no roots, both root1 and root2 will be NaN.</returns>
    public static (float root1, float root2) FindQuadraticRoots(float a, float b, float c)
    {
        float D = b*b - 4.0f * a* c;
        if(D < 0.0f) {return (float.NaN, float.NaN);}
        D = Mathf.Sqrt(D) / (2.0f * a);
        float mid = -b / (2.0f * a);
        return (mid + D, mid - D);
    }

    // NOTE: The return type ↓ should be a Either<> type, but c# doesn't support that :(
    public static (string, JumpFunction?) CalculateJumpCurve(Vector3 startPoint, Vector3 endPoint, float vertex_height, float gravity = float.NaN)
    {
        float a = float.IsFinite(gravity) ? gravity : Physics2D.gravity.y, b, velocity_x;
        float start_x = startPoint.x, start_y = startPoint.y, start_t = 0.0f;
        float vertex_x, vertex_y = vertex_height, vertex_t;
        float end_x = endPoint.x, end_y = endPoint.y, end_t;
        
        if(start_y >= vertex_y) { return ($"start_y = {start_y} >= vertex_y = {vertex_y}, unable to find valid quadratic.", null);}
        if(end_y >= vertex_y) { return ($"end_y = {end_y} >= vertex_y = {vertex_y}, unable to find valid quadratic.", null);}
        
        // the y-t function is y = a * t^2 / 2 + b * t + c, but the '/ 2' part is sort of bulky.
        a/=2;

        // let y-t to be a quadratic formula like y = a * t * t + b * t + c
        // we can know the vertex is at (-b/2a, [-b^2+4*a*c]/4a)
        // since we know a, c, and vertex.y, we can calculate how much b is equal to.
        float bb = -(4.0f * a * vertex_y - 4.0f * a * start_y);
        if(bb < 0.0f) 
        {
            return ($"Coefficient 'b' is not real number, bb = {bb}, ", null);
        }

        b = Mathf.Sqrt(bb);
        vertex_t = -b/ (2.0f * a);

        // Since a quadratic can have at most two roots, we have two candidates.
        (float end_t_candidate_1, float end_t_candidate_2) = MathUtil.FindQuadraticRoots(a, b, start_y - end_y);
        if(float.IsNaN(end_t_candidate_1))
        {
            return ($"Quadratic have zero real roots: {a} * tt + {b} * t + {start_y - end_y} = 0", null);
        }

        if(end_t_candidate_1 > vertex_t)
        {
            end_t = end_t_candidate_1;
        }
        else if(end_t_candidate_2 > vertex_t)
        {
            end_t = end_t_candidate_2;
        }
        else
        {
            return ($"Both root of end_t ({end_t_candidate_1}, {end_t_candidate_2}) is smaller than vertex_t({vertex_t})", null);
        }

        velocity_x = (start_x - end_x) / (start_t - end_t);
        vertex_x = velocity_x * vertex_t + start_x;
        
        JumpFunction ans = new JumpFunction();
        ans.xt.velocity = velocity_x;
        ans.xt.initPosition = start_x;
        ans.yt.a = a;
        ans.yt.b = b;
        ans.yt.c = start_y;
        ans.end_t = end_t;
        ans.vertex = new Vector2(vertex_x, vertex_y);
        ans.vertex_t = vertex_t;
        return ("", ans);
    }

    public static (string, JumpFunction?) CalculateJumpCurveWithRange(Vector3 startPoint, float startRangeLeft, float startRangeRight, Vector3 endPoint, float endRangeLeft, float endRangeRight, float vertex_height, float gravity = float.NaN)
    {
        float a = float.IsFinite(gravity) ? gravity : Physics2D.gravity.y, b, velocity_x;
        float start_x = startPoint.x, start_y = startPoint.y, start_t = 0.0f;
        float vertex_x, vertex_y = vertex_height, vertex_t;
        float end_x = endPoint.x, end_y = endPoint.y, end_t;
        
        if(start_y >= vertex_y) { return ($"start_y = {start_y} >= vertex_y = {vertex_y}, unable to find valid quadratic.", null);}
        if(end_y >= vertex_y) { return ($"end_y = {end_y} >= vertex_y = {vertex_y}, unable to find valid quadratic.", null);}
        
        // the y-t function is y = a * t^2 / 2 + b * t + c, but the '/ 2' part is sort of bulky.
        a/=2;

        // let y-t to be a quadratic formula like y = a * t * t + b * t + c
        // we can know the vertex is at (-b/2a, [-b^2+4*a*c]/4a)
        // since we know a, c, and vertex.y, we can calculate how much b is equal to.
        float bb = -(4.0f * a * vertex_y - 4.0f * a * start_y);
        if(bb < 0.0f) 
        {
            return ($"Coefficient 'b' is not real number, bb = {bb}, ", null);
        }

        b = Mathf.Sqrt(bb);
        vertex_t = -b/ (2.0f * a);

        // Since a quadratic can have at most two roots, we have two candidates.
        (float end_t_candidate_1, float end_t_candidate_2) = MathUtil.FindQuadraticRoots(a, b, start_y - end_y);
        if(float.IsNaN(end_t_candidate_1))
        {
            return ($"Quadratic have zero real roots: {a} * tt + {b} * t + {start_y - end_y} = 0", null);
        }

        if(end_t_candidate_1 > vertex_t)
        {
            end_t = end_t_candidate_1;
        }
        else if(end_t_candidate_2 > vertex_t)
        {
            end_t = end_t_candidate_2;
        }
        else
        {
            return ($"Both root of end_t ({end_t_candidate_1}, {end_t_candidate_2}) is smaller than vertex_t({vertex_t})", null);
        }

        float xRatio = startRangeLeft < endRangeLeft 
            ? (startRangeRight - startPoint.x) / (startRangeRight - startRangeLeft) 
            : (startRangeLeft - startPoint.x) / (startRangeRight - startRangeLeft);
        end_x = startRangeLeft < endRangeLeft
            ? endRangeRight - (endRangeRight - endRangeLeft) * xRatio
            : endRangeLeft + (endRangeRight - endRangeLeft) * xRatio;
        velocity_x = (start_x - end_x) / (start_t - end_t);
        vertex_x = velocity_x * vertex_t + start_x;
        
        JumpFunction ans = new JumpFunction();
        ans.xt.velocity = velocity_x;
        ans.xt.initPosition = start_x;
        ans.yt.a = a;
        ans.yt.b = b;
        ans.yt.c = start_y;
        ans.end_t = end_t;
        ans.vertex = new Vector2(vertex_x, vertex_y);
        ans.vertex_t = vertex_t;
        return ("", ans);
    }
}

public struct JumpFunction
{
    public struct Jump_x_t_function
    {
        public float velocity;
        public float initPosition;
        
        public readonly float On_t(float t) => velocity * t + initPosition;
    }

    public struct Jump_y_t_function
    {
        // y = a * t^2 + b * t + c
        public float a;
        public float b;
        public float c;
        public readonly float On_t(float t) => (a * t + b) * t + c;
    }

    public Jump_x_t_function xt;
    public Jump_y_t_function yt;

    // ---- extra data ----
    public float end_t;
    public Vector2 vertex;
    public float vertex_t;
}